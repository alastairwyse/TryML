import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UrlScheme } from './utilities/service-layer-interface/url-scheme';
import { HttpRequestMethod } from './utilities/service-layer-interface/http-request-method';
import { HttpUrlPrefixBuilder } from './utilities/service-layer-interface/http-url-prefix-builder';
import { HttpUrlPathAndQueryBuilder } from './utilities/service-layer-interface/http-url-path-and-query-builder';
import { ServiceLayerInterface, ServiceLayerCallResult, HttpContentType } from './utilities/service-layer-interface/service-layer-interface';
import { AngularHttpClient } from './utilities/service-layer-interface/implementations/angular-http-client';

@Injectable({
  providedIn: 'root'
})
export class DigitImageService {

  constructor(private httpClient: HttpClient) { 
  }

  /**
   * @name getDigitImage
   * @desc Retrieves the pixels data of a digit image from the remote digit image service.
   * 
   * @param {number} imageId - The numeric identifier of the image to retrieve.
   * @returns {Observable<number[][]>} An observable which publishes a 2d array containing byte values (0-255) of the pixels in the digit image.
   */
  public getDigitImage(imageId: number): Promise<ServiceLayerCallResult> {

    if (imageId < 0 || imageId > 2)
      throw new Error("Parameter 'imageId' must be between 0 and 2 inclusive.");

    let baseUri = new HttpUrlPrefixBuilder(UrlScheme.Http, "localhost", 9000, "api/DigitImages/")
    let angularHttpClient: AngularHttpClient = new AngularHttpClient(this.httpClient);
    let sli = new ServiceLayerInterface(baseUri, 10000, angularHttpClient);
    let pathAndQuery = new HttpUrlPathAndQueryBuilder(imageId.toString());
    return sli.CallServiceLayer(pathAndQuery, HttpRequestMethod.Get, null, HttpContentType.Application_Json);

    /*
    const options : object = { observe: "response", responseType: "json" };

    let result: Observable<HttpResponse<number[][]>> = this.httpClient.get<number[][]>(digitImageUri, {observe: "response", responseType: "json"});

    return result.toPromise();

    */
  }
}
