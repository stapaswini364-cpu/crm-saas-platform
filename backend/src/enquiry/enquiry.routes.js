const express = require("express");

const router = express.Router();

const enquiryController = require("./enquiry.controller");
const validateCreateEnquiry = require("./enquiry.validation");

router.post(
    "/",
    validateCreateEnquiry,
    enquiryController.createEnquiry
);

router.get(
    "/",
    enquiryController.listEnquiries
);

module.exports = router;