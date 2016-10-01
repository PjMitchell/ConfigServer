"use strict";
const router_1 = require('@angular/router');
const home_1 = require('./components/home');
const appRoutes = [
    { path: '', component: home_1.HomeComponent }
];
exports.appRoutingProviders = [];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
