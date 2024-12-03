//document.querySelectorAll('.mark-read-btn').forEach(button => {
//    button.addEventListener('click', function () {
//        var notificationId = this.getAttribute('data-id');

//        fetch('/admin/markAsRead', {
//            method: 'POST',
//            body: JSON.stringify({ id: notificationId }),
//            headers: {
//                'Content-Type': 'application/json'
//            }
//        })
//            .then(response => response.json())
//            .then(data => {
//                if (data.success) {
//                    this.closest('.notification').classList.add('read'); // Mark notification as read
//                }
//            });
//    });
//});

