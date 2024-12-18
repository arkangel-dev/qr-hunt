import { toasts } from 'svelte-toasts';

export async function Login(phonenumber) {
    phonenumber = phonenumber.toString()
    if (!IsAValidString(phonenumber)) {
        toasts.add({
            title: 'Whoops',
            description: "The phone number mustn't be empty",
            duration: 5000,
            placement: 'top-center',
            theme: 'dark',
            type: 'error',
        });
        return false;
    }
    let result = await MakePostRequest('/Api/Login', {
        username: phonenumber,
        password: phonenumber
    })
    let rjson = await result.json()
    if (result.ok) {
        localStorage.setItem('token', rjson.access_token)
        return true;
    } else {
        toasts.add({
            title: 'Login failed',
            description: rjson.message,
            duration: 5000,
            placement: 'top-center',
            theme: 'dark',
            type: 'error',
        });
        return false;
    }
}

export async function Register(phonenumber, fullname) {
    if (!IsAValidString(phonenumber) || !IsAValidString(fullname)) {
        toasts.add({
            title: 'Whoops',
            description: "The phone number and the name mustn't be empty",
            duration: 5000,
            placement: 'top-center',
            theme: 'dark',
            type: 'error',
        });
        return false;
    }
    let result = await MakePostRequest('/Api/Register', {
        fullname: fullname,
        username: phonenumber,
        password: phonenumber
    })

    const rjson = await result.json()

    if (!result.ok) {
        toasts.add({
            title: 'Whoops',
            description: rjson.message,
            duration: 5000,
            placement: 'top-center',
            theme: 'dark',
            type: 'error',
        });
        return false
    } else {
        localStorage.setItem('token', rjson.access_token)
        return true
    }
}


// const endpoint = 'http://192.168.100.4:7207';
// const websocket_url = 'ws://192.168.100.4:7207'

const endpoint = '';
const websocket_url = ''

export function GetWebSocketUrl(url)  {
    return `${websocket_url}${url}`
}

export async function MakePostRequest(url, body) {
    try {
        let headers = { 'Content-Type': 'application/json' }
        let token = localStorage.getItem('token');
        if (token) {
            headers['Authorization'] = `Bearer ${token}`
        }

        const response = await fetch(`${endpoint}${url}`, {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(body),
        });

        return response
    } catch (error) {
        console.error('Error making POST request:', error);
        return null
    }
}

export async function MakeGetRequest(url) {
    try {
        let headers = { 'Content-Type': 'application/json' }
        let token = localStorage.getItem('token');
        if (token) {
            headers['Authorization'] = `Bearer ${token}`
        }

        const response = await fetch(`${endpoint}${url}`, {
            method: 'GET',
            headers: headers
        });

        return response
    } catch (error) {
        console.error('Error making GET request:', error);
        return null
    }
}

export function DoIHaveAValidToken() {
    var token = localStorage.getItem('token')
    if (!token) return false;
    return !isTokenExpired(token);
}

function IsAValidString(str) {
    return str && str.trim()
}


function isTokenExpired(token) {
    if (!token) return true
    // Split the token into its components
    const [, payloadBase64] = token.split('.');

    // Decode the payload (Base64 URL format)
    const payloadJson = atob(payloadBase64.replace(/-/g, '+').replace(/_/g, '/'));
    const payload = JSON.parse(payloadJson);

    // Get the current time in seconds
    const currentTime = Math.floor(Date.now() / 1000);

    // Check if the token is expired
    return currentTime > payload.exp;
}