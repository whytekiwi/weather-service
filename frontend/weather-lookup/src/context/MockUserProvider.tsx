import {type User, UserContext} from './UserContext';

type MockUserProviderProps = {
    children: React.ReactNode;
    user?: User | null;
    setSelectedUser?: (user: User | null) => void;
};

const MockUserProvider = ({
  children,
  user = null,
  setSelectedUser = () => {},
}: MockUserProviderProps) => (
    <UserContext.Provider value={{selectedUser: user, setSelectedUser}}>
        {children}
    </UserContext.Provider>
);

export default MockUserProvider;