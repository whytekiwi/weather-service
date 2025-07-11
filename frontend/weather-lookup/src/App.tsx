import UserSelector from "./components/userSelector/UserSelector.tsx";
import {UserProvider} from "./context/UserContext.tsx";
import {Box, Card, Divider} from "@mui/material";
import WeatherForm from "./components/weatherForm/weatherForm.tsx";

function App() {
    return (
        <UserProvider>
            <Box sx={{
                minHeight: '100vh',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                background: 'linear-gradient(135deg, #e0e7ff 0%, #f0fdfa 100%)'
            }}>
                <Card sx={{minWidth: 400, p: 2}}>
                    <h1>Weather Look-up</h1>
                    <UserSelector/>
                    <Divider sx={{my: 2}}/>
                    <WeatherForm/>
                </Card>
            </Box>
        </UserProvider>
    )
}

export default App
