import UserSelector from "./components/userSelector/UserSelector.tsx";
import {UserProvider} from "./context/UserContext.tsx";
import {Card} from "@mui/material";
import WeatherForm from "./components/weatherForm/weatherForm.tsx";

function App() {
    return (
        <UserProvider>
            <Card>
                <h1>Weather Look-up</h1>
                <UserSelector/>
                <WeatherForm/>
            </Card>
        </UserProvider>
    )
}

export default App
