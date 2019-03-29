function detectZoom (){
    var ratio = 0,
        screen = window.screen,
        ua = navigator.userAgent.toLowerCase();

    if (window.devicePixelRatio !== undefined) {
        ratio = window.devicePixelRatio;
    }
    else if (~ua.indexOf('msie')) {
        if (screen.deviceXDPI && screen.logicalXDPI) {
            ratio = screen.deviceXDPI / screen.logicalXDPI;
        }
    }
    else if (window.outerWidth !== undefined && window.innerWidth !== undefined) {
        ratio = window.outerWidth / window.innerWidth;
    }

    if (ratio){
        ratio = Math.round(ratio * 100);
    }
    return ratio;
};
window.onresize = function(){

    if(detectZoom()>=140||detectZoom()<=110)//标准返回值为125（100%）
    {
        alert("浏览器比例将影响您的体验\n使用Ctrl+0恢复至最佳比例");
    }
}