﻿(function (global) {
    System.config({
        
        paths: {
            // paths serve as alias
            'npm:': 'https://unpkg.com/',
            'dev:': './node_modules/'
        },
        transpiler: 'ts',
        typescriptOptions: {
            // Copy of compiler options in standard tsconfig.json
            "target": "es2015",
            "module": "system",
            "moduleResolution": "node",
            "sourceMap": true,
            "emitDecoratorMetadata": true,
            "experimentalDecorators": true,
            "noImplicitAny": true,
            "suppressImplicitAnyIndexErrors": true
        },
        meta: {
            'typescript': {
                "exports": "ts"
            }
        },
        map: {
            app: 'App',

            // angular bundles
            '@angular/core': 'npm:@angular/core/bundles/core.umd.js',
            '@angular/common': 'npm:@angular/common/bundles/common.umd.js',
            '@angular/compiler': 'npm:@angular/compiler/bundles/compiler.umd.js',
            '@angular/platform-browser': 'npm:@angular/platform-browser/bundles/platform-browser.umd.js',
            '@angular/platform-browser-dynamic': 'npm:@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.js',
            '@angular/http': 'npm:@angular/http/bundles/http.umd.js',
            '@angular/router': 'npm:@angular/router/bundles/router.umd.js',
            '@angular/forms': 'npm:@angular/forms/bundles/forms.umd.js',
            '@angular/upgrade': 'npm:@angular/upgrade/bundles/upgrade.umd.js',
            // other libraries
            'rxjs': 'npm:rxjs',
            'angular2-in-memory-web-api': 'npm:angular2-in-memory-web-api',
            'jquery': 'npm:jquery/dist/jquery.min.js',
            'jquery-validation': 'npm:jquery-validation/dist/jquery.validate.js',
            'jquery-validation-unobtrusive': 'npm:jquery-validation-unobtrusive/jquery.validate.unobtrusive.js',
            'bootstrap': 'npm:bootstrap/dist/js/bootstrap.min.js',

            'ts': 'dev:plugin-typescript/lib/plugin.js',
            'typescript': 'dev:typescript/lib/typescript.js',
        },
        // packages tells the System loader how to load when no filename and/or no extension
        packages: {
            app: {
                main: 'main.ts',
                defaultExtension: 'ts'
            }
        }
    });
})(this);
