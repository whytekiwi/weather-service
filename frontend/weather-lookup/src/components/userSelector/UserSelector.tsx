import React from 'react';
import { useUser } from '../../context/UserContext';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';

type User = { id: string; name: string };

const users: User[] = [
    { id: 'e1a7c3b9d5f2a8e4c6b1d9f3a2e7c4b8', name: 'User 1' },
    { id: 'f4d2b6e8a1c9f7b3e5a2d8c6b0f1e3a7', name: 'User 2' },
    { id: 'c8f1a3d7e9b2c4a6f5d3b8e2a1c7f9d4', name: 'User 3' },
    { id: 'a9e3c7b1f6d2a8c5e4b0d7f3a2c6e8b5', name: 'User 4' },
    { id: 'd6b2e8c4a1f9b7d3e5c0a8f2b4d1c7e9', name: 'User 5' },
];

const UserSelector: React.FC = () => {
    const { selectedUser, setSelectedUser } = useUser();

    return (
        <FormControl fullWidth>
            <InputLabel id="user-select-label">Select user</InputLabel>
            <Select
                labelId="user-select-label"
                value={selectedUser?.id || ''}
                label="Select user"
                onChange={e => {
                    const user = users.find(u => u.id === e.target.value) || null;
                    setSelectedUser(user);
                }}
            >
                <MenuItem value="">
                    <em>Select user</em>
                </MenuItem>
                {users.map(user => (
                    <MenuItem key={user.id} value={user.id}>
                        {user.name}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    );
};

export default UserSelector;