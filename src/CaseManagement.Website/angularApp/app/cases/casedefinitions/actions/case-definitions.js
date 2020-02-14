export var ActionTypes;
(function (ActionTypes) {
    ActionTypes["START_SEARCH"] = "[CaseDefinitions] START_SEARCH";
    ActionTypes["COMPLETE_SEARCH"] = "[CaseDefinitions] COMPLETE_SEARCH";
    ActionTypes["START_GET"] = "[CaseDefinitions] START_GET";
    ActionTypes["COMPLETE_GET"] = "[CaseDefinitions] COMPLETE_GET";
    ActionTypes["START_GET_HISTORY"] = "[CaseDefinitions] START_GET_HISTORY";
    ActionTypes["COMPLETE_GET_HISTORY"] = "[CaseDefinitions] COMPLETE_GET_HISTORY";
})(ActionTypes || (ActionTypes = {}));
var StartFetch = (function () {
    function StartFetch(order, direction, count, startIndex, text, user) {
        this.order = order;
        this.direction = direction;
        this.count = count;
        this.startIndex = startIndex;
        this.text = text;
        this.user = user;
        this.type = ActionTypes.START_SEARCH;
    }
    return StartFetch;
}());
export { StartFetch };
var CompleteFetch = (function () {
    function CompleteFetch(content) {
        this.content = content;
        this.type = ActionTypes.COMPLETE_SEARCH;
    }
    return CompleteFetch;
}());
export { CompleteFetch };
var StartGet = (function () {
    function StartGet(id) {
        this.id = id;
        this.type = ActionTypes.START_GET;
    }
    return StartGet;
}());
export { StartGet };
var CompleteGet = (function () {
    function CompleteGet(content) {
        this.content = content;
        this.type = ActionTypes.COMPLETE_GET;
    }
    return CompleteGet;
}());
export { CompleteGet };
var StartGetHistory = (function () {
    function StartGetHistory(id) {
        this.id = id;
        this.type = ActionTypes.START_GET_HISTORY;
    }
    return StartGetHistory;
}());
export { StartGetHistory };
var CompleteGetHistory = (function () {
    function CompleteGetHistory(content) {
        this.content = content;
        this.type = ActionTypes.COMPLETE_GET_HISTORY;
    }
    return CompleteGetHistory;
}());
export { CompleteGetHistory };
//# sourceMappingURL=case-definitions.js.map