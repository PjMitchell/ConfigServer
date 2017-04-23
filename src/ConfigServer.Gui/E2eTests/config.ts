import { Config } from 'protractor';

// An example configuration file
export let config = {

  // Capabilities to be passed to the webdriver instance.
  capabilities: {
    browserName: 'chrome',
  },
  // The address of a running selenium server.
  directConnect: true,
  jasmineNodeOpts: {
    showColors: true, // Use colors in the command line report.
  },
  specs : ['specs/*.spec.js'],

};
