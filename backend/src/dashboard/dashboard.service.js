const prisma = require("../prisma/client");
const redisClient = require("../redis/redis.client");


// Dashboard KPI Summary
async function getDashboardSummary() {

    const cacheKey = "dashboard:summary";

    const cachedData = await redisClient.get(cacheKey);

    if (cachedData) {
        console.log("Summary served from Redis");
        return JSON.parse(cachedData);
    }


    const totalEnquiries = await prisma.enquiry.count();


    const totalValueResult = await prisma.enquiry.aggregate({
        _sum: {
            value: true
        }
    });


    const converted = await prisma.enquiry.count({
        where: {
            status: "CONVERTED"
        }
    });


    const result = {
        totalEnquiries,
        totalValue: totalValueResult._sum.value || 0,
        converted
    };


    await redisClient.setEx(
        cacheKey,
        300,
        JSON.stringify(result)
    );


    return result;
}



// Enquiry Pipeline with Redis Cache
async function getEnquiryPipeline() {

    const cacheKey = "dashboard:pipeline";


    const cachedData = await redisClient.get(cacheKey);

    if (cachedData) {
        console.log("Pipeline served from Redis");
        return JSON.parse(cachedData);
    }


    const pipeline = await prisma.enquiry.groupBy({

        by: ["status"],

        _count: {
            id: true
        },

        _sum: {
            value: true
        }

    });


    const result = pipeline.map(item => ({
        status: item.status,
        count: item._count.id,
        value: item._sum.value || 0
    }));


    await redisClient.setEx(
        cacheKey,
        300,
        JSON.stringify(result)
    );


    return result;
}



// Enquiries By Source with Redis Cache
async function getEnquiriesBySource() {

    const cacheKey = "dashboard:sources";


    const cachedData = await redisClient.get(cacheKey);

    if (cachedData) {
        console.log("Sources served from Redis");
        return JSON.parse(cachedData);
    }


    const sources = await prisma.enquiry.groupBy({

        by: ["source"],

        _count: {
            id: true
        }

    });


    const result = sources.map(item => ({
        source: item.source,
        count: item._count.id
    }));


    await redisClient.setEx(
        cacheKey,
        300,
        JSON.stringify(result)
    );


    return result;
}



module.exports = {
    getDashboardSummary,
    getEnquiryPipeline,
    getEnquiriesBySource
};