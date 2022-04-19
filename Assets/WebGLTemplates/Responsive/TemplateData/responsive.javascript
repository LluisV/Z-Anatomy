(function(){
    console.log('Responsive WebGL Template by SIMMER.io v2019.02.08');
    console.log('Available at: https://assetstore.unity.com/packages/tools/gui/responsive-webgl-template-117308 for free!');
    console.log('Host your WebGL Game at SIMMER.io for free!');

    const q = (selector) => document.querySelector(selector);

    const gameContainer = q('#gameContainer');

    const initialDimensions = {width: parseInt(gameContainer.style.width, 10), height: parseInt(gameContainer.style.height, 10)};
    gameContainer.style.width = '100%';
    gameContainer.style.height = '100%';

    let gCanvasElement = null;

    const getCanvasFromMutationsList = (mutationsList) => {
        for (let mutationItem of mutationsList){
            for (let addedNode of mutationItem.addedNodes){
                if (addedNode.id === '#canvas'){
                    return addedNode;
                }
            }
        }
        return null;
    }

    const setDimensions = () => {
        gameContainer.style.position = 'absolute';
        gCanvasElement.style.display = 'none';
        var winW = parseInt(window.getComputedStyle(gameContainer).width, 10);
        var winH = parseInt(window.getComputedStyle(gameContainer).height, 10);
        var scale = Math.min(winW / initialDimensions.width, winH / initialDimensions.height);
        gCanvasElement.style.display = '';
        gCanvasElement.style.width = 'auto';
        gCanvasElement.style.height = 'auto';

        var fitW = Math.round(initialDimensions.width * scale * 100) / 100;
        var fitH = Math.round(initialDimensions.height * scale * 100) / 100;

        gCanvasElement.setAttribute('width', fitW);
        gCanvasElement.setAttribute('height', fitH);
    }

    window.setDimensions = setDimensions;

    const registerCanvasWatcher = () => {
        let debounceTimeout = null;
        const debouncedSetDimensions = () => {
            if (debounceTimeout !== null) {
                clearTimeout(debounceTimeout);
            }
            debounceTimeout = setTimeout(setDimensions, 200);
        }
        window.addEventListener('resize', debouncedSetDimensions, false);
        setDimensions();
    }

    window.UnityLoader.Error.handler = function () { }

    const i = 0;
    new MutationObserver(function (mutationsList) {
        const canvas = getCanvasFromMutationsList(mutationsList)
        if (canvas){
            gCanvasElement = canvas;
            registerCanvasWatcher();

            new MutationObserver(function (attributesMutation) {
                this.disconnect();
                setTimeout(setDimensions, 1)
                q('.simmer').classList.add('hide');
            }).observe(canvas, {attributes:true});

            this.disconnect();
        }
    }).observe(gameContainer, {childList:true});

})();