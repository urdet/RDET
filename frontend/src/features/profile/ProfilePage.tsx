import { useEffect, useState } from 'react';
import { ImageUp, KeyRound, Save, UserCircle } from 'lucide-react';
import { updateCredentials, updateProfile } from '../../api';
import { Panel } from '../../shared/ui/Panel';
import { CurrentUser } from '../../types';

type ProfilePageProps = {
  currentUser: CurrentUser | null;
  onSaved: () => void;
};

export function ProfilePage({ currentUser, onSaved }: ProfilePageProps) {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [imageUrl, setImageUrl] = useState('');
  const [username, setUsername] = useState('');
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [savingProfile, setSavingProfile] = useState(false);
  const [savingCredentials, setSavingCredentials] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    if (!currentUser) return;
    setFirstName(currentUser.first_name);
    setLastName(currentUser.last_name);
    setEmail(currentUser.email ?? '');
    setPhone(currentUser.phone ?? '');
    setImageUrl(currentUser.image_url ?? '');
    setUsername(currentUser.username);
  }, [currentUser]);

  async function saveProfile() {
    setSavingProfile(true);
    setError('');
    setMessage('');
    try {
      await updateProfile({
        first_name: firstName,
        last_name: lastName,
        email: email || null,
        phone: phone || null,
        image_url: imageUrl || null,
      });
      setMessage('Profile saved.');
      await onSaved();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to save profile.');
    } finally {
      setSavingProfile(false);
    }
  }

  async function saveCredentials() {
    setSavingCredentials(true);
    setError('');
    setMessage('');
    try {
      await updateCredentials({
        username,
        current_password: currentPassword || undefined,
        new_password: newPassword || undefined,
      });
      setCurrentPassword('');
      setNewPassword('');
      setMessage('Credentials saved.');
      await onSaved();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unable to save credentials.');
    } finally {
      setSavingCredentials(false);
    }
  }

  function uploadImage(file: File | undefined) {
    if (!file) return;
    if (!file.type.startsWith('image/')) {
      setError('Please select an image file.');
      return;
    }
    if (file.size > 1_500_000) {
      setError('Image is too large. Choose an image under 1.5 MB.');
      return;
    }
    const reader = new FileReader();
    reader.onload = () => {
      setImageUrl(String(reader.result ?? ''));
      setError('');
    };
    reader.onerror = () => setError('Unable to read image.');
    reader.readAsDataURL(file);
  }

  if (!currentUser) {
    return <Panel title="Profile" icon={UserCircle}>Loading...</Panel>;
  }

  return (
    <div className="profile-layout">
      <Panel title="Profile" icon={UserCircle}>
        <div className="profile-header">
          <div className="profile-avatar">
            {imageUrl ? <img src={imageUrl} alt="" /> : <span>{initials(firstName, lastName)}</span>}
          </div>
          <div>
            <h2>{firstName} {lastName}</h2>
            <p>{currentUser.company?.name ?? 'Agency'} · {currentUser.role}</p>
          </div>
        </div>
        <label className="profile-upload">
          <ImageUp className="h-4 w-4" />
          <span>Upload profile image</span>
          <input type="file" accept="image/*" onChange={(event) => uploadImage(event.target.files?.[0])} />
        </label>
        <div className="settings-grid">
          <ProfileField label="First name" value={firstName} onChange={setFirstName} />
          <ProfileField label="Last name" value={lastName} onChange={setLastName} />
          <ProfileField label="Email" value={email} onChange={setEmail} type="email" />
          <ProfileField label="Phone" value={phone} onChange={setPhone} />
        </div>
        <div className="settings-actions">
          <button className="transfer-submit" disabled={savingProfile} onClick={saveProfile}>
            <Save className="h-4 w-4" /> {savingProfile ? 'Saving...' : 'Save profile'}
          </button>
        </div>
      </Panel>

      <Panel title="Credentials" icon={KeyRound}>
        <div className="settings-grid">
          <ProfileField label="Username" value={username} onChange={setUsername} className="settings-wide" />
          <ProfileField label="Current password" value={currentPassword} onChange={setCurrentPassword} type="password" />
          <ProfileField label="New password" value={newPassword} onChange={setNewPassword} type="password" />
        </div>
        <div className="fixed-popup-note">To change password, enter your current password and the new password. Leave both blank to change only the username.</div>
        <div className="settings-actions">
          <button className="transfer-submit" disabled={savingCredentials} onClick={saveCredentials}>
            <Save className="h-4 w-4" /> {savingCredentials ? 'Saving...' : 'Save credentials'}
          </button>
        </div>
      </Panel>

      {(message || error) && <div className={`profile-toast ${error ? 'error' : ''}`}>{error || message}</div>}
    </div>
  );
}

function ProfileField({ label, value, onChange, type = 'text', className = '' }: { label: string; value: string; onChange: (value: string) => void; type?: string; className?: string }) {
  return (
    <label className={`form-field ${className}`}>
      {label}
      <input type={type} value={value} onChange={(event) => onChange(event.target.value)} />
    </label>
  );
}

function initials(firstName: string, lastName: string) {
  return `${firstName[0] ?? ''}${lastName[0] ?? ''}`.toUpperCase() || 'U';
}
