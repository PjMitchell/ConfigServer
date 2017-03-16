import { Component } from '@angular/core';

@Component({
    selector: 'config-server-shell',
    template: `
        <nav class="navbar navbar-inverse">
            <div class="container-fluid">
                <div class="navbar-header">
                    <a class="navbar-brand" href="#">Config Server</a>
                </div>
            <ul class="nav navbar-nav">
                <li class="active"><a href="#">Home</a></li>
            </ul>
            </div>
        </nav>
        <router-outlet></router-outlet>`
})
export class AppShell { }