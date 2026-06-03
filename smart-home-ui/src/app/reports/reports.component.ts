import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, Report, AnomalyResult } from '../api.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <h2>Reports &amp; Anomaly Detection</h2>
      <p class="info-tag" role="note" *ngIf="showInfoTag">New here? Review reports first, then run anomaly detection with a valid device ID to find unusual spikes. Example: Paste a device ID like "2f94e8e1-5b63-4c3c-a581-123456789abc" and click "Detect Spikes".</p>

      <div class="report-grid">
        <div class="card">
          <h3>Device Usage</h3>
          <pre>{{ deviceReport?.content || 'No data.' }}</pre>
        </div>
        <div class="card">
          <h3>Automation Rules</h3>
          <pre>{{ rulesReport?.content || 'No data.' }}</pre>
        </div>
        <div class="card">
          <h3>User Activity</h3>
          <pre>{{ usersReport?.content || 'No data.' }}</pre>
        </div>
      </div>

      <div class="card">
        <h3>ML.NET Anomaly Detection (Spike)</h3>
        <div class="form-row">
          <input [(ngModel)]="deviceId" placeholder="Device ID (GUID)" />
          <button class="btn primary" (click)="detectSpikes()">Detect Spikes</button>
        </div>
        <table *ngIf="anomalies.length > 0">
          <thead>
            <tr><th>Anomaly?</th><th>Score</th><th>P-Value</th><th>Message</th></tr>
          </thead>
          <tbody>
            <tr *ngFor="let a of anomalies" [class.anomaly-row]="a.isAnomaly">
              <td>{{ a.isAnomaly ? '⚠️ Yes' : 'No' }}</td>
              <td>{{ a.score | number:'1.4-4' }}</td>
              <td>{{ a.pValue | number:'1.4-4' }}</td>
              <td>{{ a.message }}</td>
            </tr>
          </tbody>
        </table>
        <p *ngIf="anomalyMsg" class="empty">{{ anomalyMsg }}</p>
      </div>
    </div>
  `
})
export class ReportsComponent implements OnInit {
  deviceReport: Report | null = null;
  rulesReport: Report | null = null;
  usersReport: Report | null = null;
  deviceId = '';
  anomalies: AnomalyResult[] = [];
  anomalyMsg = '';
  showInfoTag = false;
  private readonly infoTagStorageKey = 'smart-home-info-reports-visited';

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.initializeInfoTag();
    this.api.getDeviceReport().subscribe(r => this.deviceReport = r);
    this.api.getRulesReport().subscribe(r => this.rulesReport = r);
    this.api.getUsersReport().subscribe(r => this.usersReport = r);
  }

  initializeInfoTag() {
    try {
      const visited = localStorage.getItem(this.infoTagStorageKey) === 'true';
      this.showInfoTag = !visited;
      if (!visited) {
        localStorage.setItem(this.infoTagStorageKey, 'true');
      }
    } catch {
      this.showInfoTag = true;
    }
  }

  detectSpikes() {
    if (!this.deviceId.trim()) return;
    this.anomalyMsg = '';
    this.api.getAnomalySpikes(this.deviceId.trim()).subscribe({
      next: (r) => {
        this.anomalies = r;
        if (r.length === 0) this.anomalyMsg = 'Not enough events (minimum 6 required).';
      },
      error: () => this.anomalyMsg = 'Error fetching anomaly data.'
    });
  }
}
