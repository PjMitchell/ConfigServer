import { Group } from '../interfaces/configurationSetDefintion';
import { Injectable } from '@angular/core';
import { ConfigurationClient } from '../interfaces/client';

@Injectable()
export class GroupTransitService {
    public selectedGroup: Group<string, ConfigurationClient>;
}