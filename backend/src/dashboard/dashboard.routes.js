const express = require("express");

const router = express.Router();

const dashboardController = require("./dashboard.controller");


// Dashboard KPI Summary
router.get(
    "/summary",
    dashboardController.summary
);


// Enquiry Pipeline
router.get(
    "/enquiry-pipeline",
    dashboardController.pipeline
);


// Enquiries By Source
router.get(
    "/enquiries-by-source",
    dashboardController.bySource
);


module.exports = router;