var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { MaterialModule } from '../../shared/material.module';
import { SharedModule } from '../../shared/shared.module';
import { CaseFilesRoutes } from './casefiles.routes';
import { CaseFilesEffects } from './effects/case-files';
import { AddCaseFileDialog } from './list/add-case-file-dialog';
import { ListCaseFilesComponent } from './list/list.component';
import * as reducers from './reducers';
import { CaseFilesService } from './services/casefiles.service';
import { ViewCaseFilesComponent } from './view/view.component';
import { MonacoEditorModule } from 'ngx-monaco-editor';
import { HistoryCaseFileComponent } from './history/history.component';
var CaseFilesModule = (function () {
    function CaseFilesModule() {
    }
    CaseFilesModule = __decorate([
        NgModule({
            imports: [
                CommonModule,
                NgxChartsModule,
                MonacoEditorModule.forRoot(),
                FormsModule,
                HttpClientModule,
                CaseFilesRoutes,
                MaterialModule,
                SharedModule,
                EffectsModule.forRoot([CaseFilesEffects]),
                StoreModule.forRoot(reducers.appReducer),
                StoreDevtoolsModule.instrument({
                    maxAge: 10
                })
            ],
            entryComponents: [AddCaseFileDialog],
            declarations: [ListCaseFilesComponent, AddCaseFileDialog, ViewCaseFilesComponent, HistoryCaseFileComponent],
            exports: [ListCaseFilesComponent, ViewCaseFilesComponent],
            providers: [CaseFilesService]
        })
    ], CaseFilesModule);
    return CaseFilesModule;
}());
export { CaseFilesModule };
//# sourceMappingURL=casefiles.module.js.map