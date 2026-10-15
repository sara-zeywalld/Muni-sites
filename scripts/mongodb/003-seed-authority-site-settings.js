// Run after migrations 001 and 002 with:
// mongosh "mongodb://localhost:27017" --file scripts/mongodb/003-seed-authority-site-settings.js

const databaseName = process.env.MONGO_DATABASE || "MuniSites";
const collectionName = "AuthoritySiteSettings";
const targetDb = db.getSiblingDB(databaseName);

if (!targetDb.getCollectionNames().includes(collectionName)) {
  throw new Error(
    `Collection ${databaseName}.${collectionName} does not exist. Run migrations 001 and 002 first.`
  );
}

const settings = targetDb.getCollection(collectionName);
const documents = [
  {
    authorityCode: "6500",
    authorityName: "עיריית חדרה",
    enabledCategories: ["roads", "water", "events"],
    categoryDisplayNames: {
      roads: "תחבורה וכבישים",
      water: "מים וביוב",
      events: "אירועים"
    },
    urgentCategories: ["water"],
    maxItems: 5
  },
  {
    authorityCode: "7000",
    authorityName: "עיריית דוגמה",
    enabledCategories: ["roads"],
    categoryDisplayNames: {
      roads: "כבישים"
    },
    urgentCategories: [],
    maxItems: 5
  }
];

for (const document of documents) {
  settings.updateOne(
    { authorityCode: document.authorityCode },
    {
      $set: document,
      $setOnInsert: { createdAt: new Date() }
    },
    { upsert: true }
  );

  print(`Seeded settings for authorityCode=${document.authorityCode}`);
}