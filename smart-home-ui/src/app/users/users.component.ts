import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService, User } from '../api.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="page">
      <h2>Users</h2>
      <p class="info-tag" role="note" *ngIf="showInfoTag">New here? Register people first and assign the correct role before they use the system. Example: Name "Ajay", Role "Homeowner".</p>

      <div class="card form-card">
        <h3>{{ editing ? 'Edit User' : 'Register User' }}</h3>
        <div class="form-row">
          <input [(ngModel)]="form.name" placeholder="Name" />
          <select [(ngModel)]="form.role" [disabled]="editing">
            <option value="Admin">Admin</option>
            <option value="Homeowner">Homeowner</option>
          </select>
          <button class="btn primary" (click)="save()">{{ editing ? 'Update' : 'Register' }}</button>
          <button class="btn" *ngIf="editing" (click)="cancelEdit()">Cancel</button>
        </div>
        <p class="error" *ngIf="error">{{ error }}</p>
      </div>

      <div class="card">
        <table>
          <thead>
            <tr><th>Name</th><th>Role</th><th>Actions</th></tr>
          </thead>
          <tbody>
            <tr *ngFor="let u of users">
              <td>{{ u.name }}</td>
              <td><span class="badge role">{{ u.role }}</span></td>
              <td>
                <button class="btn small" (click)="startEdit(u)">Edit</button>
                <button class="btn small danger" (click)="remove(u.id)">Delete</button>
              </td>
            </tr>
            <tr *ngIf="users.length === 0">
              <td colspan="3" class="empty">No users yet.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `
})
export class UsersComponent implements OnInit {
  users: User[] = [];
  form: Omit<User, 'id'> = { name: '', role: 'Homeowner' };
  editing = false;
  editId = '';
  error = '';
  showInfoTag = false;
  private readonly infoTagStorageKey = 'smart-home-info-users-visited';

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
    this.api.getUsers().subscribe(u => this.users = u);
  }

  save() {
    this.error = '';
    if (!this.form.name) return;
    const op = this.editing
      ? this.api.updateUser(this.editId, this.form.name)
      : this.api.addUser(this.form);
    op.subscribe({
      next: () => { this.load(); this.cancelEdit(); },
      error: (e) => this.error = e?.error?.error ?? 'An error occurred.'
    });
  }

  startEdit(u: User) {
    this.editing = true;
    this.editId = u.id;
    this.form = { name: u.name, role: u.role };
  }

  cancelEdit() {
    this.editing = false;
    this.editId = '';
    this.form = { name: '', role: 'Homeowner' };
    this.error = '';
  }

  remove(id: string) {
    this.api.deleteUser(id).subscribe(() => this.load());
  }
}
