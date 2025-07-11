# Weather Lookup App

This is a simple web app built using React + Vite + Typescript.

It allows the user to complete a web form, to look up the current weather for a `city` and `country`.

It uses a custom-built backend, found in this repo in the [backend directory](../../backend/README.md).

## Getting Started

### Install dependencies

```sh
npm install
```

### Environment Variables
Create a `.env` file in the root of the project and add the following environment variables:

```env
VITE_WEATHER_API_URL=http://localhost:7259/api/weather
```

### Run the development server

```sh
npm run dev
```

This will start the app locally. By default, it will be available at [http://localhost:5173](http://localhost:5173).

### Run tests

```sh
npm test
```

or

```sh
npx vitest
```

This will run the unit tests using [Vitest](https://vitest.dev/).

---