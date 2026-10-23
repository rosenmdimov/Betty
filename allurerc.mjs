// Allure 3 report configuration. Copied to the output folder on build;
// DIHooks runs `allure generate` from there after each test run.
export default {
  name: "Betty – Spinberry tests",
  output: "./allure-report",
  // Keeps results of previous runs, so the report can show trends and flaky tests
  historyPath: "./allure-history.jsonl",
  plugins: {
    awesome: {
      options: {
        // One self-contained index.html that opens from disk and can be shared as a file
        singleFile: true,
        reportLanguage: "en",
        groupBy: ["epic", "feature", "story"],
      },
    },
  },
};
