require("dotenv").config();

const express = require("express");
const cors = require("cors");
const helmet = require("helmet");
const morgan = require("morgan");

const dashboardRoutes = require("./src/dashboard/dashboard.routes");
const enquiryRoutes = require("./src/enquiry/enquiry.routes");

const app = express();

app.use(cors());
app.use(helmet());
app.use(morgan("dev"));
app.use(express.json());


app.use("/api/dashboard", dashboardRoutes);
app.use("/api/enquiries", enquiryRoutes);


app.get("/", (req, res) => {
    res.json({
        message: "CRM Backend Running"
    });
});


module.exports = app;