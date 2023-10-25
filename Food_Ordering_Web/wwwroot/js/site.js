const connection = new signalR.HubConnectionBuilder()
    .withUrl("/orderStatusHub")
    .build();

connection.on("ReceiveOrderUpdate", function (order) {
    // Update the UI with the new order status
    updateOrderStatusUI(order);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

let currentOrder = JSON.parse(localStorage.getItem('currentOrder'));
if (currentOrder) {
    // Show the indicator to the user
}

function updateOrderStatusUI(order) {
    // Update the UI with the new status
    let currentOrder = JSON.parse(localStorage.getItem('currentOrder'));
    currentOrder.status = order.Status;
    localStorage.setItem('currentOrder', JSON.stringify(currentOrder));
    // ... further UI updates ...
}
