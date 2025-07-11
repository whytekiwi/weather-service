/* eslint-disable react-refresh/only-export-components */
import React, {createContext, useContext, useState} from 'react';

export type User = { id: string; name: string };
type UserContextType = {
    selectedUser: User | null;
    setSelectedUser: (user: User | null) => void;
};

export const UserContext = createContext<UserContextType | undefined>(undefined);

export const UserProvider: React.FC<{ children: React.ReactNode }> = ({children}) => {
    const [selectedUser, setSelectedUser] = useState<User | null>(null);
    return (
        <UserContext.Provider value={{selectedUser, setSelectedUser}}>
            {children}
        </UserContext.Provider>
    );
};

export const useUser = () => {
    const context = useContext(UserContext);
    if (!context) throw new Error('useUser must be used within a UserProvider');
    return context;
};