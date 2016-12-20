require_relative 'common.rb'

module UnityHelper

  ############################################################

  def self.list_package_assets(dir_project, ignored_files = [])

    dir_project_plugin = resolve_path "#{dir_project}/Assets/LunarPlugin"

    files = []

    # list files
    list_assets files, dir_project_plugin, &->(file) {

      # don't include ignored files
      return false if ignored_files.include?(File.basename file)

      # the rest is fine
      return true
    }

    return files

  end

  ############################################################

  def self.list_assets(result, path, &filter)
    Dir["#{path}/*"].each do |file|

      next if File.file?(file) && File.extname(file) == '.meta'

      accepted = block_given? ? filter.call(file) : true
      next unless accepted

      if accepted
        result << file
      end

      list_assets(result, file, &filter) if File.directory?(file)
    end
  end

  ############################################################

  def self.remove_unity_asset(path)
    if (File.directory? path)
      FileUtils.rm_rf path
    else
      FileUtils.rm_f path
    end
    FileUtils.rm_f "#{path}.meta"
  end

end