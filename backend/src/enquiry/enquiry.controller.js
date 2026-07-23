const enquiryService = require("./enquiry.service");

async function createEnquiry(req, res) {
    try {

        const enquiry = await enquiryService.createEnquiry(req.body);

        return res.status(201).json(enquiry);

    } catch (error) {

        console.error(error);

        return res.status(500).json({
            message: "Failed to create enquiry"
        });
    }
}

async function listEnquiries(req, res) {

    try {

        const enquiries = await enquiryService.listEnquiries(req.query);

        return res.json(enquiries);

    } catch (error) {

        console.error(error);

        return res.status(500).json({
            message: "Failed to fetch enquiries"
        });
    }

}

module.exports = {
    createEnquiry,
    listEnquiries
};