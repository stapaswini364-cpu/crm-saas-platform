const prisma = require("../prisma/client");

async function createEnquiry(data) {
  return await prisma.enquiry.create({
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

  return await prisma.enquiry.findMany({
    where,
    orderBy: {
      createdAt: "desc",
    },
  });
}

module.exports = {
  createEnquiry,
  listEnquiries,
};