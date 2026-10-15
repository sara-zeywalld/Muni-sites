// Run after 001-create-authority-site-settings.js with:
// mongosh "mongodb://localhost:27017" --file scripts/mongodb/002-add-authority-site-settings-schema.js

const databaseName = process.env.MONGO_DATABASE || "MuniSites";
const collectionName = "AuthoritySiteSettings";
const targetDb = db.getSiblingDB(databaseName);

if (!targetDb.getCollectionNames().includes(collectionName)) {
  throw new Error(
    `Collection ${databaseName}.${collectionName} does not exist. Run migration 001 first.`
  );
}

const result = targetDb.runCommand({
  collMod: collectionName,
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: [
        "authorityCode",
        "authorityName",
        "enabledCategories",
        "categoryDisplayNames",
        "urgentCategories",
        "maxItems"
      ],
      properties: {
        authorityCode: {
          bsonType: "string",
          minLength: 1,
          description: "Authority code is required and must be a non-empty string"
        },
        authorityName: {
          bsonType: "string",
          minLength: 1,
          description: "Authority name is required and must be a non-empty string"
        },
        enabledCategories: {
          bsonType: "array",
          minItems: 1,
          items: { bsonType: "string", minLength: 1 }
        },
        categoryDisplayNames: {
          bsonType: "object",
          additionalProperties: { bsonType: "string", minLength: 1 }
        },
        urgentCategories: {
          bsonType: "array",
          items: { bsonType: "string", minLength: 1 }
        },
        maxItems: {
          bsonType: "int",
          minimum: 0,
          description: "Maximum number of returned announcements"
        },
        createdAt: { bsonType: "date" }
      }
    }
  },
  validationLevel: "strict",
  validationAction: "error"
});

if (result.ok !== 1) {
  throw new Error(`Failed to apply schema validation: ${tojson(result)}`);
}

print(`Applied schema validation to ${databaseName}.${collectionName}`);