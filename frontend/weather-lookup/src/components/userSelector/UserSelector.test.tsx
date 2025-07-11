import React from 'react';
import {render, screen, fireEvent} from '@testing-library/react';
import {describe, it, expect} from 'vitest';
import UserSelector from './UserSelector';
import {vi} from 'vitest';
import MockUserProvider from "../../context/MockUserProvider.tsx";

describe('UserSelector', () => {
    const mockSetSelectedUser = vi.fn();

    const renderWithProvider = (children: React.ReactNode) =>
        render(<MockUserProvider setSelectedUser={mockSetSelectedUser}>{children}</MockUserProvider>);


    it('renders select label', () => {
        renderWithProvider(<UserSelector/>);
        expect(screen.getByLabelText(/select user/i)).toBeInTheDocument();
    });

    it('shows users in dropdown', () => {
        renderWithProvider(<UserSelector/>);
        fireEvent.mouseDown(screen.getByLabelText(/select user/i));
        expect(screen.getByText('User 1')).toBeInTheDocument();
        expect(screen.getByText('User 2')).toBeInTheDocument();
    });

    it('calls setSelectedUser with correct user on selection', () => {
        renderWithProvider(<UserSelector/>);
        fireEvent.mouseDown(screen.getByLabelText(/select user/i));
        fireEvent.click(screen.getByText('User 1'));
        expect(mockSetSelectedUser).toHaveBeenCalledWith({id: 'e1a7c3b9d5f2a8e4c6b1d9f3a2e7c4b8', name: 'User 1'});
    });
});