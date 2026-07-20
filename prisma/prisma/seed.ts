import "dotenv/config";

import { PrismaClient } from "../generated/prisma/client";
import { PrismaPg } from "@prisma/adapter-pg";

const adapter = new PrismaPg({
  connectionString: process.env.DATABASE_URL!,
});

const prisma = new PrismaClient({
  adapter,
});

async function main() {
  console.log("🌱 Seeding Roles & Permissions...");

  const permissions = [
    { name: "users.create", description: "Create users" },
    { name: "users.read", description: "Read users" },
    { name: "users.update", description: "Update users" },
    { name: "users.delete", description: "Delete users" },

    { name: "roles.create", description: "Create roles" },
    { name: "roles.read", description: "Read roles" },
    { name: "roles.update", description: "Update roles" },
    { name: "roles.delete", description: "Delete roles" },

    { name: "permissions.read", description: "Read permissions" },

    { name: "organizations.create", description: "Create organizations" },
    { name: "organizations.read", description: "Read organizations" },
    { name: "organizations.update", description: "Update organizations" },

    { name: "vehicles.create", description: "Create vehicles" },
    { name: "vehicles.read", description: "Read vehicles" },
    { name: "vehicles.update", description: "Update vehicles" },

    { name: "bookings.create", description: "Create bookings" },
    { name: "bookings.read", description: "Read bookings" },
    { name: "bookings.update", description: "Update bookings" },

    { name: "reports.read", description: "Read reports" },
  ];

  for (const permission of permissions) {
    await prisma.permission.upsert({
      where: { name: permission.name },
      update: {},
      create: permission,
    });
  }

  const roleNames = [
    "SuperAdmin",
    "Admin",
    "Manager",
    "SalesExecutive",
    "Accountant",
    "ServiceAdvisor",
    "Viewer",
  ];

  for (const roleName of roleNames) {
    await prisma.role.upsert({
      where: { name: roleName },
      update: {},
      create: {
        name: roleName,
      },
    });
  }

  const allPermissions = await prisma.permission.findMany();

  const rolePermissionMap: Record<string, string[]> = {
    SuperAdmin: allPermissions.map((p) => p.name),

    Admin: allPermissions
      .filter((p) => p.name !== "permissions.read")
      .map((p) => p.name),

    Manager: [
      "users.read",
      "bookings.create",
      "bookings.read",
      "bookings.update",
      "reports.read",
    ],

    SalesExecutive: [
      "bookings.create",
      "bookings.read",
      "vehicles.read",
    ],

    Accountant: [
      "bookings.read",
      "reports.read",
    ],

    ServiceAdvisor: [
      "vehicles.read",
      "bookings.read",
      "bookings.update",
    ],

    Viewer: [
      "users.read",
      "vehicles.read",
      "bookings.read",
      "reports.read",
    ],
  };

  for (const roleName of Object.keys(rolePermissionMap)) {
    const role = await prisma.role.findUnique({
      where: { name: roleName },
    });

    if (!role) continue;

    for (const permissionName of rolePermissionMap[roleName]) {
      const permission = await prisma.permission.findUnique({
        where: { name: permissionName },
      });

      if (!permission) continue;

      await prisma.rolePermission.upsert({
        where: {
          roleId_permissionId: {
            roleId: role.id,
            permissionId: permission.id,
          },
        },
        update: {},
        create: {
          roleId: role.id,
          permissionId: permission.id,
        },
      });
    }
  }

  console.log("✅ Roles & Permissions seeded successfully.");
}

main()
  .catch((error) => {
    console.error(error);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });