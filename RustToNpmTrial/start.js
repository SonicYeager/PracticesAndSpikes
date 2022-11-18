#!/usr/bin/env node

const { exec } = require("child_process");

const controller = typeof AbortController !== "undefined" ? new AbortController() : { abort: () => {} };
const { signal } = controller;

exec("RustToNpmTrial", { signal }, (error, stdout, stderr) => {
  stdout && console.log(stdout);
  stderr && console.error(stderr);
  if (error !== null) {
    console.log(`exec error: ${error}`);
  }
});

process.on("SIGTERM", () => {
  controller.abort();
});
    