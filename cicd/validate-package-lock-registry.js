/**
 * Validates WebApplication/ClientApp/package-lock.json: every "resolved" tarball URL must be
 * https://registry.npmjs.org/... (file: and git+ URLs are skipped).
 * Fails with exit code 1 and a clear message if any resolved URL is not from the public npm registry host.
 *
 * Usage: node cicd/validate-package-lock-registry.js
 * (Run from repo root, or any cwd — paths are fixed relative to this file.)
 */
"use strict";

const fs = require("fs");
const path = require("path");

const repoRoot = path.join(__dirname, "..");
const lockPath = path.join(repoRoot, "WebApplication", "ClientApp", "package-lock.json");

const ALLOWED_HOST = "registry.npmjs.org";

/** @type {string[]} */
const violations = [];

/** @type {Set<string>} */
const resolved = new Set();

/**
 * @param {unknown} node
 */
function collectFromDependenciesTree(node) {
  if (node === null || node === undefined || typeof node !== "object") {
    return;
  }
  for (const pkg of Object.values(node)) {
    if (pkg === null || pkg === undefined || typeof pkg !== "object") {
      continue;
    }
    const r = pkg.resolved;
    if (typeof r === "string" && r.length > 0) {
      resolved.add(r);
    }
    if (pkg.dependencies !== undefined && pkg.dependencies !== null) {
      collectFromDependenciesTree(pkg.dependencies);
    }
  }
}

if (!fs.existsSync(lockPath)) {
  console.error(`package-lock.json not found at: ${lockPath}`);
  process.exit(1);
}

let lock;
try {
  lock = JSON.parse(fs.readFileSync(lockPath, "utf8"));
} catch (e) {
  console.error(`Failed to parse package-lock.json: ${lockPath}`);
  console.error(e);
  process.exit(1);
}

if (lock.packages !== undefined && lock.packages !== null && typeof lock.packages === "object") {
  for (const pkg of Object.values(lock.packages)) {
    if (pkg === null || pkg === undefined || typeof pkg !== "object") {
      continue;
    }
    const r = pkg.resolved;
    if (typeof r === "string" && r.length > 0) {
      resolved.add(r);
    }
  }
}

if (lock.dependencies !== undefined && lock.dependencies !== null) {
  collectFromDependenciesTree(lock.dependencies);
}

for (const raw of resolved) {
  if (raw.startsWith("file:")) {
    continue;
  }
  if (raw.startsWith("git+")) {
    continue;
  }

  if (raw.startsWith("http://")) {
    violations.push(`Insecure http:// resolved URL (use HTTPS and public npm): ${raw}`);
    continue;
  }

  if (!raw.startsWith("https://")) {
    violations.push(`Unexpected resolved URL (expected https://${ALLOWED_HOST}/ tarball): ${raw}`);
    continue;
  }

  let url;
  try {
    url = new URL(raw);
  } catch {
    violations.push(`Invalid absolute URL in resolved field: ${raw}`);
    continue;
  }

  const host = url.hostname;
  if (!host) {
    violations.push(`Resolved URL has no host: ${raw}`);
    continue;
  }

  if (host.toLowerCase() !== ALLOWED_HOST) {
    violations.push(`Disallowed registry host '${host}' (must be ${ALLOWED_HOST} only): ${raw}`);
  }
}

if (violations.length > 0) {
  console.error("");
  console.error(`package-lock.json registry validation failed: ${violations.length} issue(s).`);
  console.error(`Lockfile: ${lockPath}`);
  console.error(
    `Only HTTPS tarball URLs from ${ALLOWED_HOST} are allowed (plus ignored file:/git+ entries).`
  );
  console.error("");
  for (const v of violations) {
    console.error(`  - ${v}`);
  }
  console.error("");
  process.exit(1);
}

console.log(`package-lock.json registry OK (${resolved.size} resolved URL(s) checked).`);
