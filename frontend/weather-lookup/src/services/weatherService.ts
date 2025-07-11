import {type User} from "../context/UserContext.tsx";
import type {Weather} from "./weatherModel.ts";

export async function FetchWeather(
    city: string,
    country: string,
    selectedUser: User): Promise<Weather | null> {

    const apiUrl = `${import.meta.env.VITE_WEATHER_API_URL}?country=${country}&city=${city}`;
    const response = await fetch(apiUrl,
        {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'x-api-key': selectedUser.id
            }
        });

    if (!response.ok) {
        switch (response.status) {
            case 401:
                throw new Error('Unauthorized request');
            case 404:
                throw new Error('Weather data not found');
            case 429:
                throw new Error('Too many requests, please try again later');
            case 500:
                throw new Error('Server error, please try again later');
            default:
                throw new Error(`Unexpected error: ${response.status}`);
        }
    }

    const weather = await response.json();
    // Cache the city + country on the response, so it doesn't get updated if the user edits the inputs
    weather.city = city;
    weather.country = country;
    return weather;
}