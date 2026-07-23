function validateCreateEnquiry(req, res, next) {
    const {
        name,
        email,
        source,
        value
    } = req.body;

    if (!name || name.trim() === "") {
        return res.status(400).json({
            message: "Name is required"
        });
    }

    if (!source || source.trim() === "") {
        return res.status(400).json({
            message: "Source is required"
        });
    }

    if (email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailRegex.test(email)) {
            return res.status(400).json({
                message: "Invalid email"
            });
        }
    }

    if (value && isNaN(value)) {
        return res.status(400).json({
            message: "Value must be a number"
        });
    }

    next();
}

module.exports = validateCreateEnquiry;