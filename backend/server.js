require("dotenv").config();

const express = require("express");
const cors = require("cors");
const helmet = require("helmet");
const morgan = require("morgan");


const dashboardRoutes = require("./src/dashboard/dashboard.routes");


const app = express();


// Middleware
app.use(cors());
app.use(helmet());
app.use(morgan("dev"));
app.use(express.json());


// Routes

app.use("/api/dashboard", dashboardRoutes);


// Health check
app.get("/", (req, res) => {
    res.json({
        message: "CRM Backend Running"
    });
});


// Server

const PORT = process.env.PORT || 3000;


app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});