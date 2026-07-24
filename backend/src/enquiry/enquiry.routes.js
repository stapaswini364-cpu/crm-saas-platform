const express = require("express");

const router = express.Router();

const enquiryController = require("./enquiry.controller");
const validateCreateEnquiry = require("./enquiry.validation");

// Create Enquiry
router.post(
  "/",
  validateCreateEnquiry,
  enquiryController.createEnquiry
);

// List Enquiries
router.get(
  "/",
  enquiryController.listEnquiries
);

// Update Enquiry Stage
router.patch(
  "/:id/stage",
  enquiryController.updateStage
);

module.exports = router;