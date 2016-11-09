module SaberBase
  def ingest_test_data_to_souceone(test_data_path="")
    begin
      if system('C:\SaberAgent\Saber\AutomationFramework\ES1Automation\Main\SourceOne\CSharpNUnit\Saber\IngestTestDataToSourceOneArchiveAndIndex\bin\debug\IngestTestDataToSourceOneArchiveAndIndex.exe \"' + "#{test_data_path}" + "\"")
        puts 'Successfully ingest the test data.'
      else
        raise 'Ingest test data to SourceOne failed.'
      end
    rescue
      puts 'Error!' + $!.to_s
    end
  end
end