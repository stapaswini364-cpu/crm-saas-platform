const dashboardService = require("./dashboard.service");


// Dashboard KPI Summary
exports.summary = async (req, res) => {
    try {

        const data = await dashboardService.getDashboardSummary();

        res.json(data);

    } catch (error) {

        console.error(error);

        res.status(500).json({
            message: "Dashboard summary failed",
            error: error.message
        });

    }
};



// Enquiry Pipeline
exports.pipeline = async (req, res) => {
    try {

        const data = await dashboardService.getEnquiryPipeline();

        res.json(data);

    } catch (error) {

        console.error(error);

        res.status(500).json({
            message: "Enquiry pipeline failed",
            error: error.message
        });

    }
};



// Enquiries By Source
exports.bySource = async (req, res) => {
    try {

        const data = await dashboardService.getEnquiriesBySource();

        res.json(data);

    } catch (error) {

        console.error(error);

        res.status(500).json({
            message: "Enquiries by source failed",
            error: error.message
        });

    }
};