import UserSelector from "./components/userSelector/UserSelector.tsx";
import {UserProvider, useUser} from "./context/UserContext.tsx";
import {Card} from "@mui/material";

function AppContent() {
    const {selectedUser} = useUser();

    return (
        <Card>
            <h1>Hello world</h1>
            <UserSelector/>
            {selectedUser && <div>Selected user Id = {selectedUser?.id}</div>}
        </Card>
    )
}

// Wrap the AppContent with UserProvider to provide user context
function App() {
    return (
        <UserProvider>
            <AppContent/>
        </UserProvider>
    )
}

export default App
