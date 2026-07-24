const prisma = require("../prisma/client");

async function createEnquiry(data) {
  // Duplicate email/phone check
  const duplicate = await prisma.enquiry.findFirst({
    where: {
      OR: [
        ...(data.email ? [{ email: data.email }] : []),
        ...(data.phone ? [{ phone: data.phone }] : []),
      ],
    },
  });

  if (duplicate) {
    const error = new Error("Enquiry with this email or phone already exists.");
    error.status = 409;
    throw error;
  }

  return prisma.enquiry.create({
    data: {
      name: data.name,
      email: data.email || null,
      phone: data.phone || null,
      owner: data.owner || null,
      service: data.service || null,
      source: data.source,
      status: data.status || "NEW",
      value: Number(data.value || 0),
    },
  });
}

async function listEnquiries(filters) {
  const where = {};

  if (filters.owner) where.owner = filters.owner;
  if (filters.source) where.source = filters.source;
  if (filters.service) where.service = filters.service;
  if (filters.status) where.status = filters.status;

  if (filters.startDate || filters.endDate) {
    where.createdAt = {};

    if (filters.startDate) {
      where.createdAt.gte = new Date(filters.startDate);
    }

    if (filters.endDate) {
      where.createdAt.lte = new Date(filters.endDate);
    }
  }

  return prisma.enquiry.findMany({
    where,
    orderBy: {
      createdAt: "desc",
    },
  });
}

async function changeStage(id, status) {
  const enquiry = await prisma.enquiry.findUnique({
    where: { id },
  });

  if (!enquiry) {
    const error = new Error("Enquiry not found");
    error.status = 404;
    throw error;
  }

  const updated = await prisma.enquiry.update({
    where: { id },
    data: {
      status,
    },
  });

  await prisma.enquiryAudit.create({
    data: {
      enquiryId: id,
      oldStatus: enquiry.status,
      newStatus: status,
    },
  });

  return updated;
}

module.exports = {
  createEnquiry,
  listEnquiries,
  changeStage,
};