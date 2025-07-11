import {TextField, Button, Box, FormHelperText, Alert} from '@mui/material';
import {useEffect, useState, useTransition} from "react";
import type {Weather} from "../../services/weatherModel.ts";
import {FetchWeather} from "../../services/weatherService.ts";
import {useUser} from "../../context/UserContext.tsx";

const WeatherForm = () => {
    const {selectedUser} = useUser();

    useEffect(() => {
        setError(null);
    }, [selectedUser]);

    const [city, setCity] = useState('');
    const [county, setCountry] = useState('');
    const [isPending, startTransition] = useTransition();
    const [error, setError] = useState<string | null>(null);
    const [weatherData, setWeatherData] = useState<Weather | null>(null);

    const submitFormAction = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        // Reset previous state
        setError(null);
        setWeatherData(null);

        if (!selectedUser) {
            setError("Please select a user before requesting weather.");
            return;
        }
        startTransition(async () => {
            try {
                const weatherData = await FetchWeather(city, county, selectedUser);
                if (!weatherData) {
                    setError(`No weather data found for ${city}, ${county}.`);
                }
                setWeatherData(weatherData);
            } catch (err: unknown) {
                if (err instanceof Error) {
                    setError(err.message);
                } else {
                    setError("An unexpected error occurred while fetching weather data.");
                }
            }
        })
    };

    return (
        <Box
            component="form"
            onSubmit={submitFormAction}
        >
            <TextField
                label="City"
                name="city"
                onChange={(e) => setCity(e.target.value)}
                required
                disabled={isPending}
            />
            <TextField
                label="Country"
                name="country"
                onChange={(e) => setCountry(e.target.value)}
                required
                disabled={isPending}
            />
            <Button type="submit" disabled={isPending}>Submit</Button>
            {error && (
                <FormHelperText error>{error}</FormHelperText>
            )}
            {weatherData && (
                <Alert severity="success" sx={{mt: 2}}>
                    Weather for {weatherData.city}, {weatherData.country}: {weatherData.description}
                </Alert>
            )}
        </Box>
    )
};

export default WeatherForm;