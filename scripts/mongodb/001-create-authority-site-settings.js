// Run with: mongosh "mongodb://localhost:27017" --file scripts/mongodb/001-create-authority-site-settings.js

const databaseName = process.env.MONGO_DATABASE || "MuniSites";
const collectionName = "AuthoritySiteSettings";
const targetDb = db.getSiblingDB(databaseName);

if (!targetDb.getCollectionNames().includes(collectionName)) {
  targetDb.createCollection(collectionName);
  print(`Created collection ${databaseName}.${collectionName}`);
} else {
  print(`Collection ${databaseName}.${collectionName} already exists`);
}

const settings = targetDb.getCollection(collectionName);

settings.createIndex(
  { authorityCode: 1 },
  { name: "ux_authorityCode", unique: true }
);

print(`Ready for settings documents in ${databaseName}.${collectionName}`);