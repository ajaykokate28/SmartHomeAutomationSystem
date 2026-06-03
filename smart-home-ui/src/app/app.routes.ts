import { Routes } from '@angular/router';
import { DevicesComponent } from './devices/devices.component';
import { UsersComponent } from './users/users.component';
import { AutomationRulesComponent } from './automation-rules/automation-rules.component';
import { ReportsComponent } from './reports/reports.component';

export const routes: Routes = [
  { path: '', redirectTo: 'devices', pathMatch: 'full' },
  { path: 'devices', component: DevicesComponent },
  { path: 'users', component: UsersComponent },
  { path: 'rules', component: AutomationRulesComponent },
  { path: 'reports', component: ReportsComponent },
];
