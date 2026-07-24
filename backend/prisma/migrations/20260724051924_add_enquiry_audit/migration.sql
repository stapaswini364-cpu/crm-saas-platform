-- CreateTable
CREATE TABLE "EnquiryAudit" (
    "id" TEXT NOT NULL,
    "enquiryId" TEXT NOT NULL,
    "oldStatus" TEXT NOT NULL,
    "newStatus" TEXT NOT NULL,
    "changedAt" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT "EnquiryAudit_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE INDEX "EnquiryAudit_enquiryId_idx" ON "EnquiryAudit"("enquiryId");

-- CreateIndex
CREATE INDEX "EnquiryAudit_changedAt_idx" ON "EnquiryAudit"("changedAt");

-- AddForeignKey
ALTER TABLE "EnquiryAudit" ADD CONSTRAINT "EnquiryAudit_enquiryId_fkey" FOREIGN KEY ("enquiryId") REFERENCES "Enquiry"("id") ON DELETE CASCADE ON UPDATE CASCADE;
