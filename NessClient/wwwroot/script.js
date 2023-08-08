document.addEventListener("DOMContentLoaded", () => {
    const uploadForm = document.getElementById("uploadForm");
    const uploadButton = document.getElementById("uploadButton");
    const getClientsButton = document.getElementById("getClientsButton");
    const clientsDisplay = document.getElementById("clientsDisplay");
    const addClientForm = document.getElementById("addClientForm");
    const addClientButton = document.getElementById("addClientButton");
    const getGeoInfoForm = document.getElementById("getGeoInfoForm");
    const getGeoInfoButton = document.getElementById("getGeoInfoButton");
    const removeClientForm = document.getElementById("removeClientForm");
    const removeClientButton = document.getElementById("removeClientButton");

    const baseUrl = "https://localhost:44317";

    uploadButton.addEventListener("click", async () => {
        const formData = new FormData(uploadForm);
        try {
            const response = await fetch(`${baseUrl}/api/Client/UploadFile`, {
                method: "POST",
                body: formData,
            });
            const result = await response.text();
            alert(result);
        } catch (error) {
            console.error(error);
        }
    });

    getClientsButton.addEventListener("click", async () => {
        try {
            const response = await fetch(`${baseUrl}/api/Client`, {
                method: "GET",
            });
            const clients = await response.json();
            clientsDisplay.textContent = JSON.stringify(clients, null, 2);
        } catch (error) {
            console.error(error);
        }
    });

    addClientButton.addEventListener("click", async () => {
        const fullName = document.getElementById("fullName").value;
        const id = document.getElementById("id").value;
        const ipAddress = document.getElementById("ipAddress").value;
        const phoneNumber = document.getElementById("phoneNumber").value;

        const clientData = {
            fullName: fullName,
            id: parseInt(id),
            ipAddress: ipAddress,
            phoneNumber: phoneNumber,
        };

        try {
            const response = await fetch(`${baseUrl}/api/Client`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(clientData),
            });
            const result = await response.text();
            alert(result);
        } catch (error) {
            console.error(error);
        }
    });

    getGeoInfoButton.addEventListener("click", async () => {
        const geoIpAddress = document.getElementById("geoIpAddress").value;

        try {
            const response = await fetch(`${baseUrl}/api/Client/GeoInfo?ipAddress=${geoIpAddress}`, {
                method: "GET",
            });
            const geoInfo = await response.json();
            alert(JSON.stringify(geoInfo, null, 2));
        } catch (error) {
            console.error(error);
        }
    });

    removeClientButton.addEventListener("click", async () => {
        const removeClientId = document.getElementById("removeClientId").value;

        try {
            const response = await fetch(`${baseUrl}/api/Client/${removeClientId}`, {
                method: "DELETE",
            });
            const result = await response.text("Client Deleted seccsussfuly");
            alert(result);
        } catch (error) {
            console.error(error);
        }
    });
});