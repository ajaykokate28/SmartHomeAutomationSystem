import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, AutomationRule } from '../api.service';

@Component({
  selector: 'app-automation-rules',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <h2>Automation Rules</h2>
      <p class="info-tag" role="note" *ngIf="showInfoTag">New here? Create a rule by defining a condition and action, then optionally add a schedule. Example: If "Motion detected", then "Turn on hallway light" at "19:00".</p>

      <div class="card form-card">
        <h3>{{ editing ? 'Edit Rule' : 'Create Rule' }}</h3>
        <div class="form-row">
          <input [(ngModel)]="form.name" placeholder="Rule Name" />
          <input [(ngModel)]="form.condition" placeholder="Condition (e.g. motion detected)" />
          <input [(ngModel)]="form.action" placeholder="Action (e.g. turn on light)" />
          <input [(ngModel)]="form.schedule" placeholder="Schedule HH:mm (optional)" />
          <button class="btn primary" (click)="save()">{{ editing ? 'Update' : 'Create' }}</button>
          <button class="btn" *ngIf="editing" (click)="cancelEdit()">Cancel</button>
        </div>
      </div>

      <div class="card">
        <table>
          <thead>
            <tr><th>Name</th><th>Condition</th><th>Action</th><th>Schedule</th><th>Actions</th></tr>
          </thead>
          <tbody>
            <tr *ngFor="let r of rules">
              <td>{{ r.name }}</td>
              <td>{{ r.condition }}</td>
              <td>{{ r.action }}</td>
              <td>{{ r.schedule || '—' }}</td>
              <td>
                <button class="btn small" (click)="startEdit(r)">Edit</button>
                <button class="btn small danger" (click)="remove(r.id)">Delete</button>
              </td>
            </tr>
            <tr *ngIf="rules.length === 0">
              <td colspan="5" class="empty">No rules yet.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class AutomationRulesComponent implements OnInit {
  rules: AutomationRule[] = [];
  form: Omit<AutomationRule, 'id'> = { name: '', condition: '', action: '', schedule: '' };
  editing = false;
  editId = '';
  showInfoTag = false;
  private readonly infoTagStorageKey = 'smart-home-info-rules-visited';

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
    this.api.getRules().subscribe(r => this.rules = r);
  }

  save() {
    if (!this.form.name || !this.form.condition || !this.form.action) return;
    const payload = { ...this.form, schedule: this.form.schedule || undefined };
    const op = this.editing
      ? this.api.updateRule(this.editId, payload)
      : this.api.addRule(payload);
    op.subscribe(() => { this.load(); this.cancelEdit(); });
  }

  startEdit(r: AutomationRule) {
    this.editing = true;
    this.editId = r.id;
    this.form = { name: r.name, condition: r.condition, action: r.action, schedule: r.schedule ?? '' };
  }

  cancelEdit() {
    this.editing = false;
    this.editId = '';
    this.form = { name: '', condition: '', action: '', schedule: '' };
  }

  remove(id: string) {
    this.api.deleteRule(id).subscribe(() => this.load());
  }
}
