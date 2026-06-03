import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, Device } from '../api.service';

@Component({
  selector: 'app-devices',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <h2>Devices</h2>
      <p class="info-tag" role="note" *ngIf="showInfoTag">New here? Start by adding a device with a name, type, and status so you can automate it later. Example: Name "Living Room Light", Type "Light", Status "Off".</p>

      <!-- Add / Edit Form -->
      <div class="card form-card">
        <h3>{{ editing ? 'Edit Device' : 'Add Device' }}</h3>
        <div class="form-row">
          <input [(ngModel)]="form.name" placeholder="Name" />
          <input [(ngModel)]="form.type" placeholder="Type (Light / Thermostat …)" />
          <select [(ngModel)]="form.status">
            <option value="On">On</option>
            <option value="Off">Off</option>
            <option value="Idle">Idle</option>
          </select>
          <button class="btn primary" (click)="save()">{{ editing ? 'Update' : 'Add' }}</button>
          <button class="btn" *ngIf="editing" (click)="cancelEdit()">Cancel</button>
        </div>
      </div>

      <!-- Device Table -->
      <div class="card">
        <table>
          <thead>
            <tr><th>Name</th><th>Type</th><th>Status</th><th>Actions</th></tr>
          </thead>
          <tbody>
            <tr *ngFor="let d of devices">
              <td>{{ d.name }}</td>
              <td>{{ d.type }}</td>
              <td><span class="badge" [class]="d.status.toLowerCase()">{{ d.status }}</span></td>
              <td>
                <button class="btn small" (click)="startEdit(d)">Edit</button>
                <button class="btn small danger" (click)="remove(d.id)">Delete</button>
              </td>
            </tr>
            <tr *ngIf="devices.length === 0">
              <td colspan="4" class="empty">No devices yet.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class DevicesComponent implements OnInit {
  devices: Device[] = [];
  form: Omit<Device, 'id'> = { name: '', type: '', status: 'Off' };
  editing = false;
  editId = '';
  showInfoTag = false;
  private readonly infoTagStorageKey = 'smart-home-info-devices-visited';

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.initializeInfoTag();
    this.load();
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

  load() {
    this.api.getDevices().subscribe(d => this.devices = d);
  }

  save() {
    if (!this.form.name || !this.form.type) return;
    const op = this.editing
      ? this.api.updateDevice(this.editId, this.form)
      : this.api.addDevice(this.form);
    op.subscribe(() => { this.load(); this.cancelEdit(); });
  }

  startEdit(d: Device) {
    this.editing = true;
    this.editId = d.id;
    this.form = { name: d.name, type: d.type, status: d.status };
  }

  cancelEdit() {
    this.editing = false;
    this.editId = '';
    this.form = { name: '', type: '', status: 'Off' };
  }

  remove(id: string) {
    this.api.deleteDevice(id).subscribe(() => this.load());
  }
}
