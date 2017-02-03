
var edge = require('edge')

var bridgeFile = 'mx840Bridge.dll';
var ip_mx840B = "169.254.190.130";
var port = 5001;
var invoke = edge.func({
    assemblyFile: bridgeFile,
    typeName: 'mx840Bridge.Bridge',
    methodName: 'Invoke'
});

invoke({ operation: "open", host: ip_mx840B, port:port }, function (error, result) {
    if (error) {
        console.log("ERROR:" + error.message);
    }
    else {
        setInterval(function () {
            invoke({ operation: "read", host: ip_mx840B, port: port }, function (error, result) {
                if (error)
                    console.log("ERROR:" + error.message);
                else {
                    console.log(result);
                } 
            });
        }, 4000);
    }    
        
});



//invoke({ method: "test" }, function (error, result) {
//    console.log("Success open...")

//});
