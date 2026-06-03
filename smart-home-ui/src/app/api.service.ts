import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface Device {
  id: string;
  name: string;
  type: string;
  status: string;
}

export interface User {
  id: string;
  name: string;
  role: string;
}

export interface AutomationRule {
  id: string;
  name: string;
  condition: string;
  action: string;
  schedule?: string;
}

export interface Report {
  type: string;
  content: string;
}

export interface AnomalyResult {
  isAnomaly: boolean;
  score: number;
  pValue: number;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Devices
  getDevices(): Observable<Device[]> {
    return this.http.get<Device[]>(`${this.base}/devices`);
  }
  addDevice(device: Omit<Device, 'id'>): Observable<unknown> {
    return this.http.post(`${this.base}/devices`, device);
  }
  updateDevice(id: string, device: Omit<Device, 'id'>): Observable<unknown> {
    return this.http.put(`${this.base}/devices/${id}`, device);
  }
  deleteDevice(id: string): Observable<unknown> {
    return this.http.delete(`${this.base}/devices/${id}`);
  }

  // Users
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.base}/users`);
  }
  addUser(user: Omit<User, 'id'>): Observable<unknown> {
    return this.http.post(`${this.base}/users`, user);
  }
  updateUser(id: string, name: string): Observable<unknown> {
    return this.http.put(`${this.base}/users/${id}`, { name });
  }
  deleteUser(id: string): Observable<unknown> {
    return this.http.delete(`${this.base}/users/${id}`);
  }

  // Automation Rules
  getRules(): Observable<AutomationRule[]> {
    return this.http.get<AutomationRule[]>(`${this.base}/automationrules`);
  }
  addRule(rule: Omit<AutomationRule, 'id'>): Observable<unknown> {
    return this.http.post(`${this.base}/automationrules`, rule);
  }
  updateRule(id: string, rule: Omit<AutomationRule, 'id'>): Observable<unknown> {
    return this.http.put(`${this.base}/automationrules/${id}`, rule);
  }
  deleteRule(id: string): Observable<unknown> {
    return this.http.delete(`${this.base}/automationrules/${id}`);
  }

  // Reports
  getDeviceReport(): Observable<Report> {
    return this.http.get<Report>(`${this.base}/reports/devices`);
  }
  getRulesReport(): Observable<Report> {
    return this.http.get<Report>(`${this.base}/reports/rules`);
  }
  getUsersReport(): Observable<Report> {
    return this.http.get<Report>(`${this.base}/reports/users`);
  }
  getAnomalySpikes(deviceId: string): Observable<AnomalyResult[]> {
    return this.http.get<AnomalyResult[]>(`${this.base}/reports/anomaly/spikes/${deviceId}`);
  }
}
