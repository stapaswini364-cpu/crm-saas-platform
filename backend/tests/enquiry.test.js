const request = require("supertest");
const app = require("../app");
const prisma = require("../src/prisma/client");
const redisClient = require("../src/redis/redis.client");

const testEmail = `testuser_${Date.now()}@gmail.com`;
const testPhone = `999999${Date.now().toString().slice(-4)}`;

describe("Enquiry API Tests", () => {

  let createdId;


  afterAll(async () => {

    if (redisClient.isOpen) {
      await redisClient.quit();
    }

    await prisma.$disconnect();

  });


  test("GET / should return backend running", async () => {

    const response = await request(app).get("/");

    expect(response.statusCode).toBe(200);

    expect(response.body.message)
      .toBe("CRM Backend Running");

  });


  test("Create enquiry successfully", async () => {

    const response = await request(app)
      .post("/api/enquiries")
      .send({
        name: "Test User",
        email: testEmail,
        phone: testPhone,
        owner: "Admin",
        service: "CRM",
        source: "Website",
        value: 10000
      });


    expect(response.statusCode).toBe(201);

    expect(response.body.email)
      .toBe(testEmail);


    createdId = response.body.id;

  });


  test("Duplicate email should fail", async () => {

    const response = await request(app)
      .post("/api/enquiries")
      .send({
        name: "Duplicate User",
        email: testEmail,
        phone: "8888888888",
        owner: "Admin",
        service: "CRM",
        source: "Website",
        value: 5000
      });


    expect(response.statusCode)
      .toBe(409);


    expect(response.body.message)
      .toContain("already exists");

  });


  test("Update enquiry stage and create audit trail", async () => {

    const response = await request(app)
      .patch(`/api/enquiries/${createdId}/stage`)
      .send({
        status: "QUALIFIED"
      });


    expect(response.statusCode)
      .toBe(200);


    expect(response.body.status)
      .toBe("QUALIFIED");


    const audit = await prisma.enquiryAudit.findFirst({
      where: {
        enquiryId: createdId,
        oldStatus: "NEW",
        newStatus: "QUALIFIED"
      }
    });


    expect(audit)
      .not
      .toBeNull();

  });


});