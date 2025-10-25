/**
 * Standup Issue Creator
 * ---------------------
 * Reads markdown files in /docs/standups/
 * and creates or updates GitHub issues accordingly.
 * Adds the "daily" label automatically.
 */

import fs from "fs";
import path from "path";
import { Octokit } from "@octokit/rest";
import * as core from "@actions/core";
import * as github from "@actions/github";

const octokit = new Octokit({ auth: process.env.GITHUB_TOKEN });
const { owner, repo } = github.context.repo;

// 🏷️ Ensure "daily" label exists
async function ensureLabelExists(labelName, color = "0e8a16") {
  try {
    await octokit.issues.getLabel({ owner, repo, name: labelName });
    core.info(`Label "${labelName}" already exists.`);
  } catch {
    core.info(`Creating missing label "${labelName}"...`);
    await octokit.issues.createLabel({ owner, repo, name: labelName, color });
  }
}

// 🧩 Parse the markdown content into structured data
function parseStandup(mdContent) {
  const titleMatch = mdContent.match(/^## Standup - ([\d/]+)/m);
  const title = titleMatch ? `Standup - ${titleMatch[1]}` : "Standup Report";

  const statusMatch = mdContent.match(/\*\*Status:\*\*\s*(.+)/);
  const status = statusMatch ? statusMatch[1].trim() : "No status provided";

  // Capture @usernames (letters, numbers, underscores, hyphens)
  const members = [...mdContent.matchAll(/(@[A-Za-z0-9_-]+)\s*<br>/g)].map(m => m[1]);

  const data = {
    yesterday: [],
    today: [],
    obstacles: [],
  };

  for (const member of members) {
    const sectionRegex = new RegExp(`${member}([\\s\\S]*?)(?=@|$)`, "g");
    const sectionMatch = mdContent.match(sectionRegex);
    const section = sectionMatch ? sectionMatch[0] : "";

    const yesterday = section.match(/\*\*What did I do yesterday\?\*\*<br>(.*?)<br>\*\*/s);
    const today = section.match(/\*\*What did I do today\?\*\*<br>(.*?)<br>\*\*/s);
    const obstacles = section.match(/\*\*What obstacles do I have\?\*\*<br>(.*?)$/s);

    data.yesterday.push(yesterday ? yesterday[1].trim() : "-");
    data.today.push(today ? today[1].trim() : "-");
    data.obstacles.push(obstacles ? obstacles[1].trim() : "-");
  }

  // Build Markdown table
  const header = `| Question | ${members.join(" | ")} |\n| - | ${members.map(() => "-").join(" | ")} |`;
  const rows = [
    `| **What did I do yesterday?** | ${data.yesterday.join(" | ")} |`,
    `| **What did I do today?** | ${data.today.join(" | ")} |`,
    `| **What obstacles do I have?** | ${data.obstacles.join(" | ")} |`
  ];

  const body = `
## Daily standup Report:

**Status:** ${status}

${header}
${rows.join("\n")}
`;

  return { title, body };
}

// 🚀 Main
async function main() {
  await ensureLabelExists("daily");

  const dir = path.join(process.cwd(), "docs/standups");
  const files = fs.readdirSync(dir).filter(f => f.endsWith(".md"));

  for (const file of files) {
    const filePath = path.join(dir, file);
    const content = fs.readFileSync(filePath, "utf8");
    const { title, body } = parseStandup(content);

    core.info(`📄 Processing daily file: ${file}`);
    core.info(`🔎 Looking for existing issue titled "${title}"...`);

    // Check if an issue with the same title already exists
    const existingIssues = await octokit.paginate(octokit.issues.listForRepo, {
      owner,
      repo,
      state: "open",
      per_page: 100,
    });

    const existing = existingIssues.find(i => i.title === title);

    if (existing) {
      core.info(`✅ Found existing issue #${existing.number}. Updating...`);
      await octokit.issues.update({
        owner,
        repo,
        issue_number: existing.number,
        body,
        labels: ["daily"],
      });
    } else {
      core.info(`🆕 No existing issue found. Creating new one.`);
      await octokit.issues.create({
        owner,
        repo,
        title,
        body,
        labels: ["daily"],
      });
    }
  }
}

main().catch(err => core.setFailed(err.message));
