require_relative 'common.rb'

module UnityHelper

  ############################################################

  def self.list_package_assets dir_project, ignored_files = []

    dir_project_assets = resolve_path "#{dir_project}/Assets"
    dir_project_assets_editor = resolve_path "#{dir_project_assets}/Editor/Lunar"
    dir_project_assets_plugins = resolve_path "#{dir_project_assets}/Plugins/Lunar"

    files = []

    # list editor files
    list_assets files, dir_project_assets_editor, &->(file) {

                          # don't include ignored files
                          return false if ignored_files.include?(File.basename file)

                          # the rest is fine
                          return true
                        }

    # list runtime files
    list_assets files, dir_project_assets_plugins, &->(file) {

                          # don't include ignored files
                          return false if ignored_files.include?(File.basename file)

                          # include all directories
                          return true if File.directory?(file)

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