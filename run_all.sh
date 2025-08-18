#!/bin/sh

dotnet AdvertisingPlatforms.Web.dll &
dotnet DataGenerator/DataGenerator.dll &

wait
