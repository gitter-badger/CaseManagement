import { Action } from '@ngrx/store';
import { SearchCasePlanInstanceResult } from '../models/searchcaseplaninstanceresult.model';

export enum ActionTypes {
    START_SEARCH = "[CaseInstances] START_SEARCH",
    COMPLETE_SEARCH = "[CaseInstances] COMPLETE_SEARCH"
}

export class StartSearch implements Action {
    readonly type = ActionTypes.START_SEARCH
    constructor(public id: string, public startIndex: number, public count: number, public order: string, public direction: string) { }
}

export class CompleteSearch implements Action {
    readonly type = ActionTypes.COMPLETE_SEARCH;
    constructor(public content: SearchCasePlanInstanceResult) { }
}

export type ActionsUnion = StartSearch | CompleteSearch;