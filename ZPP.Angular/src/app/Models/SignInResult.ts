import { JsonWebToken } from './JsonWebToken';

export class SignInResult{
    success : boolean;
    message : string;
    token : JsonWebToken;
}